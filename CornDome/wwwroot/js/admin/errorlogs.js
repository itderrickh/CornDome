const exceptionModal = document.getElementById('exceptionModal');

exceptionModal.addEventListener('show.bs.modal', function (event) {
    const button = event.relatedTarget;
    const exception = button.getAttribute('data-exception');

    document.getElementById('exceptionText').textContent = exception;
});

function changePageSize(size) {
    const url = new URL(window.location.href);

    url.searchParams.set('pageNumber', '1');
    url.searchParams.set('pageSize', size);

    window.location.href = url.toString();
}