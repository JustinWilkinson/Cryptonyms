window.codenames = {
    setFocus: function (id) {
        let element = document.getElementById(id);
        if (element) {
            element.focus();
        }
    },
    blurElement: function (id) {
        let element = document.getElementById(id);
        if (element) {
            element.blur();
        }
    },
    scrollToBottomOfElement: function (id) {
        let element = document.getElementById(id);
        if (element) {
            element.scrollTop = element.scrollHeight;
        }
    },
    appendContent: function (id, content) {
        let element = document.getElementById(id);
        if (element) {
            element.insertAdjacentHTML('beforeend', content);
        }
    },
    replaceContent: function (id, regex, newContent) {
        let element = document.getElementById(id);
        if (element) {            
            element.innerText = element.innerText.replace(new RegExp(regex), newContent);
        }
    },
    replaceAllContent: function (id, newContent) {
        let element = document.getElementById(id);
        if (element) {
            element.innerText = newContent;
        }
    },
    slideToggle: function (id) {
        $(`#${id}`).slideToggle('slow');
    },
    runAfterTimeout: function(functionToRun, param, timeout) {
        setTimeout(() => this[functionToRun](param), timeout);
    },
    gameCompletedAnimation: function (winningTeam) {
        let masterTimeline = gsap.timeline({
            paused: true,
            onComplete: () => $('.party-popper-container').html('')
        });

        $('.party-popper-container').append(new Array(400).join(`<span class="balloon balloon-${winningTeam}"></span>`));
        let random = (min, max) => min + (Math.random() * (max - min));
        let parentHeight = $('.party-popper-container').parent().height();
        let parentWidth = $('.party-popper-container').parent().width();
        let animationTimeline = gsap.timeline();
        $('.balloon').each(function (i, balloon) {
            var tl = gsap.timeline()
                .to(balloon, random(0.7, 2), { x: random(-parentWidth/2, parentWidth/2), y: random(-parentHeight / 1.5, -parentHeight * 2), rotation: random(-360, 360), ease: "power3.easeOut" }, random(0, 1))
            animationTimeline.add(tl, 0);
        });

        masterTimeline.add(animationTimeline);
        masterTimeline.play(0);
    }
}