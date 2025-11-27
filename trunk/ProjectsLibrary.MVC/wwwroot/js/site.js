const toggleButton = document.getElementById('toggle-btn')
const sidebar = document.getElementById('sidebar')

document.addEventListener("DOMContentLoaded", () => {
    const isClosed = localStorage.getItem('sidebarClosed');
    if (isClosed === 'true') {
        sidebar.classList.add('close');
    } else {
        sidebar.classList.remove('close');
    }
});

function toggleSidebar() {
    sidebar.classList.toggle('close');
    toggleButton.classList.toggle('rotate');

    const isClosed = sidebar.classList.contains('close');
    localStorage.setItem('sidebarClosed', isClosed);
}