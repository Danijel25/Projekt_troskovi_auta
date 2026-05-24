// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

(() => {
	const debounceMs = 250;

	const buildSearchUrl = (baseUrl, query) => {
		if (!query) {
			return baseUrl;
		}

		const separator = baseUrl.includes("?") ? "&" : "?";
		return `${baseUrl}${separator}query=${encodeURIComponent(query)}`;
	};

	const fetchResults = (url) =>
		fetch(url, {
			headers: {
				"X-Requested-With": "XMLHttpRequest"
			}
		}).then((response) => {
			if (!response.ok) {
				throw new Error(`Search request failed: ${response.status}`);
			}
			return response.text();
		});

	const initAjaxSearch = (container) => {
		const input = container.querySelector("[data-search-input]");
		const clearButton = container.querySelector("[data-search-clear]");
		const status = container.querySelector("[data-search-status]");
		const targetSelector = container.getAttribute("data-search-target");
		const searchUrl = container.getAttribute("data-search-url");

		if (!input || !targetSelector || !searchUrl) {
			return;
		}

		const target = document.querySelector(targetSelector);
		if (!target) {
			return;
		}

		let timeoutId = null;
		let lastQuery = "";

		const updateStatus = (message) => {
			if (status) {
				status.textContent = message;
			}
		};

		const runSearch = (query) => {
			updateStatus("Searching...");
			fetchResults(buildSearchUrl(searchUrl, query))
				.then((markup) => {
					target.innerHTML = markup;
					updateStatus("");
				})
				.catch(() => {
					updateStatus("Search failed. Please try again.");
				});
		};

		const scheduleSearch = () => {
			const query = input.value.trim();
			if (query === lastQuery) {
				return;
			}

			lastQuery = query;
			if (clearButton) {
				clearButton.disabled = query.length === 0;
			}

			if (timeoutId) {
				window.clearTimeout(timeoutId);
			}

			timeoutId = window.setTimeout(() => runSearch(query), debounceMs);
		};

		input.addEventListener("input", scheduleSearch);
		input.addEventListener("search", scheduleSearch);
		input.addEventListener("keydown", (event) => {
			if (event.key === "Escape") {
				input.value = "";
				scheduleSearch();
			}
		});

		if (clearButton) {
			clearButton.addEventListener("click", () => {
				input.value = "";
				input.focus();
				scheduleSearch();
			});
			clearButton.disabled = input.value.trim().length === 0;
		}
	};

	const initAutocompleteDropdown = (container) => {
		const input = container.querySelector("[data-lookup-input]");
		const hiddenInput = container.querySelector("[data-lookup-value]");
		const list = container.querySelector("[data-lookup-list]");
		const status = container.querySelector("[data-lookup-status]");
		const toggle = container.querySelector("[data-lookup-toggle]");
		const lookupUrl = container.getAttribute("data-lookup-url");
		const minChars = parseInt(container.getAttribute("data-min-chars") || "1", 10);
		const limit = parseInt(container.getAttribute("data-limit") || "25", 10);

		if (!input || !hiddenInput || !list || !lookupUrl) {
			return;
		}

		let items = [];
		let activeIndex = -1;
		let timeoutId = null;
		let lastQuery = null;
		let controller = null;

		const setStatus = (message) => {
			if (status) {
				status.textContent = message || "";
			}
		};

		const closeList = () => {
			list.hidden = true;
			list.innerHTML = "";
			activeIndex = -1;
			input.setAttribute("aria-expanded", "false");
		};

		const updateActive = (nextIndex) => {
			const options = Array.from(list.querySelectorAll("[data-index]"));
			if (!options.length) {
				activeIndex = -1;
				return;
			}

			const clamped = Math.max(0, Math.min(nextIndex, options.length - 1));
			options.forEach((option, index) => {
				const isActive = index === clamped;
				option.classList.toggle("is-active", isActive);
				option.setAttribute("aria-selected", isActive ? "true" : "false");
				if (isActive) {
					option.scrollIntoView({ block: "nearest" });
				}
			});
			activeIndex = clamped;
		};

		const renderList = () => {
			list.innerHTML = "";
			if (!items.length) {
				closeList();
				return;
			}

			items.forEach((item, index) => {
				const option = document.createElement("li");
				option.className = "auto-lookup-option";
				option.setAttribute("role", "option");
				option.setAttribute("data-index", index.toString());
				option.setAttribute("aria-selected", "false");

				const label = document.createElement("span");
				label.className = "auto-lookup-label";
				label.textContent = item.label || "";
				option.appendChild(label);

				if (item.hint) {
					const hint = document.createElement("span");
					hint.className = "auto-lookup-hint";
					hint.textContent = item.hint;
					option.appendChild(hint);
				}

				list.appendChild(option);
			});

			list.hidden = false;
			input.setAttribute("aria-expanded", "true");
			activeIndex = -1;
		};

		const selectItem = (index) => {
			const item = items[index];
			if (!item) {
				return;
			}

			input.value = item.label || "";
			hiddenInput.value = item.value || "";
			closeList();
			setStatus("");
		};

		const fetchOptions = (query, force) => {
			const trimmed = query.trim();
			if (!force && trimmed.length < minChars) {
				closeList();
				setStatus(minChars > 1 ? `Type at least ${minChars} characters.` : "");
				return;
			}

			if (controller) {
				controller.abort();
			}
			controller = new AbortController();

			const url = new URL(lookupUrl, window.location.origin);
			url.searchParams.set("query", trimmed);
			url.searchParams.set("limit", limit.toString());

			setStatus("Searching...");
			fetch(url.toString(), {
				headers: {
					"X-Requested-With": "XMLHttpRequest"
				},
				signal: controller.signal
			})
				.then((response) => {
					if (!response.ok) {
						throw new Error(`Lookup request failed: ${response.status}`);
					}
					return response.json();
				})
				.then((data) => {
					items = Array.isArray(data) ? data : [];
					renderList();
					setStatus(items.length ? "" : "No matches found.");
				})
				.catch((error) => {
					if (error.name === "AbortError") {
						return;
					}
					setStatus("Search failed. Please try again.");
					closeList();
				});
		};

		const scheduleFetch = () => {
			const query = input.value;
			hiddenInput.value = "";
			if (query === lastQuery) {
				return;
			}

			lastQuery = query;
			if (timeoutId) {
				window.clearTimeout(timeoutId);
			}
			timeoutId = window.setTimeout(() => fetchOptions(query, false), debounceMs);
		};

		input.addEventListener("input", scheduleFetch);
		input.addEventListener("keydown", (event) => {
			if (event.key === "ArrowDown") {
				event.preventDefault();
				updateActive(activeIndex + 1);
			} else if (event.key === "ArrowUp") {
				event.preventDefault();
				updateActive(activeIndex - 1);
			} else if (event.key === "Enter") {
				if (!list.hidden && activeIndex >= 0) {
					event.preventDefault();
					selectItem(activeIndex);
				}
			} else if (event.key === "Escape") {
				closeList();
			}
		});

		input.addEventListener("focus", () => {
			if (items.length) {
				list.hidden = false;
				input.setAttribute("aria-expanded", "true");
			}
		});

		if (toggle) {
			toggle.addEventListener("click", () => {
				if (!list.hidden) {
					closeList();
					return;
				}

				fetchOptions(input.value, true);
				input.focus();
			});
		}

		list.addEventListener("mousedown", (event) => {
			const option = event.target.closest("[data-index]");
			if (!option) {
				return;
			}
			event.preventDefault();
			selectItem(Number(option.getAttribute("data-index")));
		});

		document.addEventListener("click", (event) => {
			if (!container.contains(event.target)) {
				closeList();
			}
		});
	};

	const initCarFiles = (container) => {
		const list = container.querySelector("[data-file-list]");
		const status = container.querySelector("[data-file-status]");
		const count = container.querySelector("[data-file-count]");
		const dropzoneElement = container.querySelector("[data-dropzone]");
		const uploadUrl = container.getAttribute("data-upload-url");
		const listUrl = container.getAttribute("data-list-url");
		const deleteUrl = container.getAttribute("data-delete-url");

		if (!list || !listUrl) {
			return;
		}

		const setStatus = (message) => {
			if (status) {
				status.textContent = message || "";
			}
		};

		const setCount = (value) => {
			if (count) {
				count.textContent = value.toString();
			}
		};

		const formatFileSize = (size) => {
			if (size < 1024) {
				return `${size} B`;
			}
			if (size < 1024 * 1024) {
				return `${(size / 1024).toFixed(1)} KB`;
			}
			return `${(size / (1024 * 1024)).toFixed(1)} MB`;
		};

		const formatDate = (value) => {
			const date = new Date(value);
			if (Number.isNaN(date.getTime())) {
				return "";
			}
			return date.toLocaleString();
		};

		const buildFileItem = (file) => {
			const item = document.createElement("li");
			item.className = "auto-list-item";

			const primary = document.createElement("div");
			primary.className = "auto-item-primary";

			const link = document.createElement("a");
			link.className = "auto-file-name";
			link.href = file.url || "#";
			link.target = "_blank";
			link.rel = "noopener";
			link.textContent = file.fileName || "Attachment";

			const deleteButton = document.createElement("button");
			deleteButton.type = "button";
			deleteButton.className = "auto-btn-delete";
			deleteButton.textContent = "Delete";
			deleteButton.setAttribute("data-delete-file", file.id);

			primary.appendChild(link);
			primary.appendChild(deleteButton);

			const meta = document.createElement("div");
			meta.className = "auto-file-meta";
			const sizeLabel = formatFileSize(file.fileSize || 0);
			const dateLabel = formatDate(file.uploadedAt);
			meta.textContent = dateLabel ? `${sizeLabel} | ${dateLabel}` : sizeLabel;

			item.appendChild(primary);
			item.appendChild(meta);

			return item;
		};

		const renderFiles = (files) => {
			list.innerHTML = "";
			setCount(files.length);

			if (!files.length) {
				const empty = document.createElement("li");
				empty.className = "auto-file-empty";
				empty.textContent = "No attachments uploaded yet.";
				list.appendChild(empty);
				return;
			}

			files.forEach((file) => {
				list.appendChild(buildFileItem(file));
			});
		};

		const fetchFiles = () => {
			setStatus("Loading files...");
			return fetch(listUrl, {
				headers: {
					"X-Requested-With": "XMLHttpRequest"
				}
			})
				.then((response) => {
					if (!response.ok) {
						throw new Error(`File list failed: ${response.status}`);
					}
					return response.json();
				})
				.then((data) => {
					const files = Array.isArray(data) ? data : [];
					renderFiles(files);
					setStatus(files.length ? "" : "Ready");
				})
				.catch(() => {
					setStatus("Failed to load attachments.");
				});
		};

		const handleDelete = (fileId) => {
			if (!deleteUrl) {
				return;
			}
			setStatus("Deleting file...");
			fetch(`${deleteUrl}/${fileId}`, {
				method: "DELETE",
				headers: {
					"X-Requested-With": "XMLHttpRequest"
				}
			})
				.then((response) => {
					if (!response.ok) {
						throw new Error(`Delete failed: ${response.status}`);
					}
					return fetchFiles();
				})
				.catch(() => {
					setStatus("Failed to delete file.");
				});
		};

		list.addEventListener("click", (event) => {
			const target = event.target.closest("[data-delete-file]");
			if (!target) {
				return;
			}
			const fileId = Number(target.getAttribute("data-delete-file"));
			if (Number.isNaN(fileId)) {
				return;
			}
			handleDelete(fileId);
		});

		fetchFiles();

		if (dropzoneElement && window.Dropzone && uploadUrl) {
			Dropzone.autoDiscover = false;
			const dropzone = new Dropzone(dropzoneElement, {
				url: uploadUrl,
				paramName: "file",
				uploadMultiple: true,
				parallelUploads: 4,
				maxFilesize: 25
			});

			dropzone.on("queuecomplete", () => {
				fetchFiles();
			});

			dropzone.on("error", () => {
				setStatus("Upload failed. Please try again.");
			});

			dropzone.on("sending", () => {
				setStatus("Uploading...");
			});
		}
	};

	document.addEventListener("DOMContentLoaded", () => {
		document.querySelectorAll("[data-ajax-search]").forEach(initAjaxSearch);
		document.querySelectorAll("[data-autocomplete-dropdown]").forEach(initAutocompleteDropdown);
		document.querySelectorAll("[data-car-files]").forEach(initCarFiles);
	});
})();
