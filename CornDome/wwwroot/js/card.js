function showFeedbackForm() {
    document.getElementById("feedback-form").style.display = "block";
}

function hideFeedbackForm() {
    document.getElementById("feedback-form").style.display = "none";
}

function submitFeedback() {
    var text = document.getElementById("feedback-input").value;
    var card = document.getElementById("feedback-card").value;
    var revision = document.getElementById("feedback-revision").value;

    hideFeedbackForm();

    var feedbackForm = {
        feedback: text,
        cardId: parseInt(card, 10),
        revisionId: parseInt(revision, 10)
    };

    fetch('/Card?handler=Feedback', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        credentials: 'same-origin',
        body: JSON.stringify(feedbackForm)
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                document.getElementById("feedback-success").style.display = "block";
                document.getElementById("feedback-error").style.display = "none";
            } else {
                document.getElementById("feedback-error").style.display = "block";
                document.getElementById("feedback-success").style.display = "none";
            }

            document.getElementById("feedback-result").style.display = "block";
            document.getElementById("feedback-section").style.display = "none";
            document.getElementById("feedback-input").value = "";

            setTimeout(function () {
                document.getElementById("feedback-result").style.display = "none";
                document.getElementById("feedback-section").style.display = "block";
            }, 5000);
        })
        .catch(error => console.error('Error:', error));
}