using CarExpenses.DAL;
using CarExpenses.Model.Models;
using CarExpenses.Web.Api.Dtos;
using CarExpenses.Web.Api.Mapping;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarExpenses.Web.Controllers.Api;

[Authorize]
[ApiController]
[Route("api/cars/{carId:int}/files")]
public sealed class CarFilesApiController : ControllerBase
{
    private readonly CarExpesesDbContext dbContext;
    private readonly IWebHostEnvironment webHostEnvironment;

    public CarFilesApiController(CarExpesesDbContext dbContext, IWebHostEnvironment webHostEnvironment)
    {
        this.dbContext = dbContext;
        this.webHostEnvironment = webHostEnvironment;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CarFileDto>>> GetAll(int carId)
    {
        if (!await dbContext.Cars.AnyAsync(car => car.Id == carId))
        {
            return NotFound();
        }

        var files = await dbContext.CarFiles
            .Where(file => file.CarId == carId)
            .AsNoTracking()
            .OrderByDescending(file => file.UploadedAt)
            .ToListAsync();

        var result = files
            .Select(file => DtoMapping.ToDto(file, BuildFileUrl(file.RelativePath)))
            .ToList();

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<IEnumerable<CarFileDto>>> Upload(int carId)
    {
        if (!await dbContext.Cars.AnyAsync(car => car.Id == carId))
        {
            return NotFound();
        }

        if (Request.Form.Files.Count == 0)
        {
            return BadRequest("No files were uploaded.");
        }

        var rootPath = GetWebRootPath();
        var storagePath = Path.Combine(rootPath, "uploads", "cars", carId.ToString());
        Directory.CreateDirectory(storagePath);

        var uploadedAt = DateTime.UtcNow;
        var storedFiles = new List<CarFile>();

        foreach (var file in Request.Form.Files)
        {
            if (file.Length <= 0)
            {
                continue;
            }

            var originalName = Path.GetFileName(file.FileName);
            var extension = Path.GetExtension(originalName);
            var storedName = $"{Guid.NewGuid():N}{extension}";
            var relativePath = $"uploads/cars/{carId}/{storedName}";
            var physicalPath = Path.Combine(storagePath, storedName);

            await using (var stream = System.IO.File.Create(physicalPath))
            {
                await file.CopyToAsync(stream);
            }

            storedFiles.Add(new CarFile
            {
                CarId = carId,
                OriginalFileName = originalName,
                StoredFileName = storedName,
                ContentType = file.ContentType,
                FileSize = file.Length,
                RelativePath = relativePath,
                UploadedAt = uploadedAt
            });
        }

        if (storedFiles.Count == 0)
        {
            return BadRequest("Uploaded files were empty.");
        }

        dbContext.CarFiles.AddRange(storedFiles);
        await dbContext.SaveChangesAsync();

        var result = storedFiles
            .Select(file => DtoMapping.ToDto(file, BuildFileUrl(file.RelativePath)))
            .ToList();

        return Ok(result);
    }

    [HttpDelete("{fileId:int}")]
    public async Task<IActionResult> Delete(int carId, int fileId)
    {
        var file = await dbContext.CarFiles
            .FirstOrDefaultAsync(item => item.CarId == carId && item.Id == fileId);

        if (file is null)
        {
            return NotFound();
        }

        var rootPath = GetWebRootPath();
        var relativePath = file.RelativePath.TrimStart('/', '\\');
        var physicalPath = Path.Combine(rootPath, relativePath.Replace('/', Path.DirectorySeparatorChar));

        if (System.IO.File.Exists(physicalPath))
        {
            System.IO.File.Delete(physicalPath);
        }

        dbContext.CarFiles.Remove(file);
        await dbContext.SaveChangesAsync();

        return NoContent();
    }

    private string GetWebRootPath()
    {
        return string.IsNullOrWhiteSpace(webHostEnvironment.WebRootPath)
            ? Path.Combine(webHostEnvironment.ContentRootPath, "wwwroot")
            : webHostEnvironment.WebRootPath;
    }

    private static string BuildFileUrl(string relativePath)
    {
        var cleaned = relativePath.TrimStart('/', '\\').Replace("\\", "/");
        return $"/{cleaned}";
    }
}
