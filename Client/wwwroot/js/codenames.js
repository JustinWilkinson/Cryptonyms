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
    gameCompletedAnimation: function (winningTeam, showAssassin) {
        $('.content').hide();

        $('.main').prepend('<div class="animation-container"></div>');
        $('.animation-container').append('<div id="Balloons" class="h-100 w-100">' + new Array(400).join(`<span class="balloon balloon-${winningTeam}"></span>`) + '</div>');
        let random = (min, max) => min + (Math.random() * (max - min));
        let parentHeight = 0.9 * $('.main').height();
        let parentWidth = 0.9 * $('.main').width();

        let masterTimeline = gsap.timeline({
            paused: true,
            onComplete: () => {
                $('.animation-container').remove();
                $('.content').fadeIn('slow');
            }
        });

        if (showAssassin) {
            $('#Balloons').hide();
            $('.animation-container').prepend('<img id="Assassin" class="img-endgame img-assassin" src="/images/Assassin.png" width="0" height="0" />');

            let spyColour = winningTeam === 'blue' ? 'red' : 'blue';
            $('.animation-container').prepend(`<img id="Spy" class="img-endgame img-spy" src="/images/${spyColour}-man.png" width="0" height="0" />`);
            $('.animation-container').prepend('<img id="Bullet" class="img-endgame img-bullet" src="/images/bullet.png" width="10" height="5" />')

            masterTimeline.add(gsap.timeline().to('#Spy', {
                duration: 1,
                width: '20vh',
                height: '40vh',
                opacity: 1,
            }));
            masterTimeline.add(gsap.timeline().to('#Assassin', {
                duration: 2,
                width: '20vh',
                height: '40vh',
                rotationY: 360,
                opacity: 1,
                onComplete: () => $('#Bullet').css('opacity', 1)
            }));
            masterTimeline.add(gsap.timeline().to('#Bullet', {
                duration: 0.4,
                opacity: 1,
                x: -Math.max(document.documentElement.clientWidth, window.innerWidth || 0) / 2.6,
                onComplete: () => setTimeout(() => $('#Bullet').remove(), 50)
            }));
            masterTimeline.add(gsap.timeline().to('#Spy', {
                delay: 0.1,
                duration: 0.5,
                width: '20vh',
                height: '40vh',
                opacity: 0
            }));
            masterTimeline.add(gsap.timeline().to('#Assassin', {
                delay: 1,
                duration: 1,
                opacity: 0,
                onComplete: () => {
                    $('#Assassin').remove();
                    $('#Spy').remove();
                    $('#Balloons').fadeIn('fast');
                }
            }));
        }

        let balloonsTimeline = gsap.timeline();
        $('.balloon').each(function (i, balloon) {
            let options = {
                x: random(-parentWidth / 2, parentWidth / 2),
                y: random(-parentHeight / 1.5, -parentHeight * 2),
                rotation: random(-360, 360), ease: "power3.easeOut"
            };
            let tl = gsap.timeline().to(balloon, random(0.7, 2), options, random(0, 1))
            balloonsTimeline.add(tl, 0);
        });

        masterTimeline.add(balloonsTimeline);
        masterTimeline.play(0);
    }
}