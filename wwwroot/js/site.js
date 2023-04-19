// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

document.addEventListener('DOMContentLoaded', function () {
    console.log("js working");
    navScrollAnimation();
    navOpenAnimation();
    faqAnimation();
});

// GENERAL - NAV SCROLL ANIMATION
function navScrollAnimation() {

    const header = document.querySelector('header.main-header');
    const hamburgerLines = document.querySelectorAll('.menu line');
    const logo = document.querySelector('.logo');
    const deskNavLinks = document.querySelectorAll('.desk-nav-link');

    // tl.reversed is default to false
    const tl = new TimelineMax({ paused: true, reversed: false });

    tl.fromTo(header, 0.15, { background: 'none', opacity: 1 }, { background: 'white', opacity: 0.9 })
        .fromTo(logo, 0.2, { color: 'white' }, { color: 'black' }, '-=0.1')
        .fromTo(hamburgerLines, 0.2, { stroke: 'white' }, { stroke: 'black' }, '-=0.2')
        .fromTo(deskNavLinks, 0.2, { color: 'white' }, { color: 'black' }, '-=0.3');

    window.onscroll = () => {
        if (window.scrollY > 30) {
            if (tl.paused() || tl.reversed()) {
                tl.play();
            }

        } else {
            if (!tl.paused() && !tl.reversed()) {
                tl.reverse();
            }
        }
    }
}

// NAV OPEN ANIMATION
function navOpenAnimation() {

    const hamburger = document.querySelector('.menu');
    const hamburgerLines = document.querySelectorAll('.menu line');
    const navOpen = document.querySelector('.nav-open');
    const navBreaks = document.querySelectorAll('hr');
    const navLinks = document.querySelectorAll('.burger-nav-link');

    // tl.reversed is default to false
    const tl = new TimelineMax({ paused: true, reversed: true });

    tl.to(navOpen, 0.5, { x: 0 })
        .fromTo(navBreaks, 0.5, { opacity: 0, y: 10 }, { opacity: 1, y: 0 }, '-=0.1')
        .fromTo(navLinks, 0.5, { opacity: 0, x: 15 }, { opacity: 1, x: 0 }, '-=0.5')
        .fromTo(hamburgerLines, 0.2, { stroke: 'white' }, { stroke: 'black' }, '-=1');

    hamburger.addEventListener('click', () => {
        tl.reversed() ? tl.play() : tl.reverse();
    })
}

// HOME PAGE - FAQ ANIMATION
function faqAnimation() {

    const accordion_questions = document.querySelectorAll(".faq-accordion-question");

    // checks a click event and extends the accordion according to the size needed
    accordion_questions.forEach((accordion_question) => {
        accordion_question.addEventListener('click', () => {
            const height = accordion_question.nextElementSibling.scrollHeight;
            console.log(height);
            accordion_question.classList.toggle('active-header');
            if (accordion_question.classList.contains('active-header')) {
                accordion_question.nextElementSibling.style.maxHeight = `${height}px`;
            } else {
                accordion_question.nextElementSibling.style.maxHeight = '0px';
            }
        });
    });

}
