var days = document.querySelectorAll(".day");
var starts = document.querySelectorAll(".startTime");
var ends = document.querySelectorAll(".endTime");

function toggleDisplays() {
    days.forEach((day) => {
        var dayValue = day.dataset.day;

        if (day.checked) {
            starts.forEach((start) => {
                if (start.dataset.day === dayValue) {
                    start.style.display = "inline";
                }
            });

            ends.forEach((end) => {
                if (end.dataset.day === dayValue) {
                    end.style.display = "inline";
                }
            });
        } else {
            starts.forEach((start) => {
                if (start.dataset.day === dayValue) {
                    start.style.display = "none";
                }
            });

            ends.forEach((end) => {
                if (end.dataset.day === dayValue) {
                    end.style.display = "none";
                }
            });
        }
    });
}

toggleDisplays();