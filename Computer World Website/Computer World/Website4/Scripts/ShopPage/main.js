// SHOW MENU
const showMenu = (toggleId, navbarId,bodyId) =>{
    const toggle = document.getElementById(toggleId),
    navbar = document.getElementById(navbarId),
    bodypadding = document.getElementById(bodyId)

    if(toggle && navbar){
        toggle.addEventListener('click', ()=>{
            // APARECER MENU
            navbar.classList.toggle('show')
            // ROTATE TOGGLE
            toggle.classList.toggle('rotate')
            // PADDING BODY
            bodypadding.classList.toggle('expander')
        })
    }
}
showMenu('nav-toggle','navbar','row')

// LINK ACTIVE COLOR
const linkColor = document.querySelectorAll('.nav__link2');   
function colorLink(){
    linkColor.forEach(l => l.classList.remove('active2'));
    this.classList.add('active2');
}

linkColor.forEach(l => l.addEventListener('click', colorLink));

