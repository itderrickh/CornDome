document.addEventListener("DOMContentLoaded", async () => {
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/notices")
        .withAutomaticReconnect()
        .build();

    connection.on("MaintenanceUpdated", notice => {
        showMaintenanceNotice(notice);
    });

    connection.on("MaintenanceCancelled", id => {
        removeMaintenanceNotice(id);
    });

    connection.on("MaintenanceStarted", notice => {
        showMaintenanceNotice(notice);
    });

    connection.on("MaintenanceEnded", id => {
        removeMaintenanceNotice(id);
    });

    try {
        await connection.start();

        const response = await fetch("/api/notices/active");

        if (response.ok) {
            const notice = await response.json();

            if (notice)
                showMaintenanceNotice(notice);
        }
    } catch (error) {
        console.error("Maintenance connection failed:", error);
    }
});

function showMaintenanceNotice(notice) {
    let banner = document.getElementById("maintenanceNotice");

    if (!banner) {
        banner = document.createElement("div");
        banner.id = "maintenanceNotice";

        const noticeContainer = document.getElementById("notice");
        (noticeContainer || document.body).appendChild(banner);
    }

    banner.dataset.noticeId = notice.id;

    const start = new Date(
        notice.scheduledStart.endsWith("Z")
            ? notice.scheduledStart
            : notice.scheduledStart + "Z"
    );

    banner.className = "alert alert-warning text-center rounded-0 mb-0";
    banner.innerHTML = `
        <strong>⚠️ ${escapeHtml(notice.title)}</strong>
        <div>${escapeHtml(notice.message)}</div>
        <div class="mt-1">
            <strong>Maintenance begins in: <span id="maintenanceCountdown"></span></strong>
        </div>
        <button type="button" class="btn-close ms-2"
            aria-label="Dismiss"></button>
    `;

    banner.querySelector(".btn-close").addEventListener("click", dismissMaintenanceNotice);

    // Clear any previous countdown
    if (window.maintenanceCountdownInterval) {
        clearInterval(window.maintenanceCountdownInterval);
    }

    function updateCountdown() {
        const countdown = document.getElementById("maintenanceCountdown");
        if (!countdown) {
            clearInterval(window.maintenanceCountdownInterval);
            return;
        }

        const remaining = start.getTime() - Date.now();

        if (remaining <= 0) {
            countdown.textContent = "Starting now!";
            clearInterval(window.maintenanceCountdownInterval);
            return;
        }

        const days = Math.floor(remaining / (1000 * 60 * 60 * 24));
        const hours = Math.floor((remaining / (1000 * 60 * 60)) % 24);
        const minutes = Math.floor((remaining / (1000 * 60)) % 60);
        const seconds = Math.floor((remaining / 1000) % 60);

        countdown.textContent =
            `${days > 0 ? days + "d " : ""}${hours}h ${minutes}m ${seconds}s`;
    }

    updateCountdown();
    window.maintenanceCountdownInterval = setInterval(updateCountdown, 1000);
}

function removeMaintenanceNotice(id) {
    const notice = document.getElementById("maintenanceNotice");

    if (notice && notice.dataset.noticeId === String(id)) {
        notice.remove();
    }
}

function dismissMaintenanceNotice() {
    document.getElementById("maintenanceNotice")?.remove();
}

function escapeHtml(value) {
    const element = document.createElement("div");
    element.textContent = value ?? "";
    return element.innerHTML;
}
