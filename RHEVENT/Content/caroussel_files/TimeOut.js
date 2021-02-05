


let b1 = document.getElementById('b1');
let b2 = document.getElementById('b2');
let timeoutId;

b1.addEventListener('click', message);
b2.addEventListener('click', stopDelai);

function message() {
    timeoutId = setTimeout(alert, 2000, 'Message d\'alerte après 2 secondes');
}
function stopDelai() {
    clearTimeout(timeoutId);
}