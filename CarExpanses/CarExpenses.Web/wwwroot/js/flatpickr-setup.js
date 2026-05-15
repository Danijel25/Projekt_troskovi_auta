(function () {
    const flatpickrGlobal = window.flatpickr;
    if (!flatpickrGlobal) {
        return;
    }

    const browserLocale = (navigator.language || "en").toLowerCase();
    const isCroatian = browserLocale.startsWith("hr");
    const localeConfig = isCroatian && flatpickrGlobal.l10ns && flatpickrGlobal.l10ns.hr
        ? flatpickrGlobal.l10ns.hr
        : flatpickrGlobal.l10ns.default;

    const displayFormat = isCroatian ? "d.m.Y H:i" : "m/d/Y h:i K";
    const dateFormat = "Y-m-d\\TH:i";

    const normalizeValue = (value) => {
        if (!value) {
            return value;
        }

        if (value.includes("T")) {
            return value;
        }

        if (value.includes(" ")) {
            return value.replace(" ", "T");
        }

        if (/^\d{4}-\d{2}-\d{2}$/.test(value)) {
            return `${value}T00:00`;
        }

        return value;
    };

    const inputs = document.querySelectorAll(
        'input[type="date"], input[type="datetime-local"], input[data-flatpickr="true"]'
    );

    inputs.forEach((input) => {
        if (input._flatpickr) {
            return;
        }

        if (input.type !== "text") {
            input.type = "text";
        }

        const normalizedValue = normalizeValue(input.value);
        if (normalizedValue !== input.value) {
            input.value = normalizedValue;
        }

        const altInputClass = `${input.className} flatpickr-alt-input`.trim();

        flatpickrGlobal(input, {
            allowInput: true,
            altInput: true,
            altFormat: displayFormat,
            altInputClass: altInputClass,
            dateFormat: dateFormat,
            disableMobile: true,
            enableTime: true,
            locale: localeConfig,
            minuteIncrement: 5,
            time_24hr: isCroatian
        });
    });
})();
