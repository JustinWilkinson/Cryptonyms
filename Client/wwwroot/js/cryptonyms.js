window.cryptonyms = {
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
    runAfterTimeout: function (functionToRun, param, timeout) {
        setTimeout(() => this[functionToRun](param), timeout);
    },
    initialiseGamesDataTable: function () {
        $('#GamesTable').DataTable({
            retrieve: true,
            paging: true,
            pageLength: 10,
            lengthMenu: [[10, 25, 50, 100, -1], [10, 25, 50, 100, "All"]],
            ordering: false,
            columnDefs: [
                {
                    targets: 0,
                    searchable: true
                },
                {
                    targets: '_all',
                    searchable: false
                }
            ],
            language: {
                info: "Showing _START_ to _END_ of _TOTAL_ games."
            }
        });
    },
    initialiseWordsDataTable: function () {
        $('#WordsTable').DataTable({
            ajax: {
                url: 'api/Word/List',
                dataSrc: function (res) {
                    return res;
                }
            },
            columns: [
                {
                    data: 'Text',
                    className: 'align-middle'
                },
                {
                    data: 'Editable',
                    render: function (data, index, row) {
                        if (data) {
                            return `<button class="btn btn-danger adjustable-font-size-small" onclick="cryptonyms.removeWord('${row.Text}');">Remove</button>`;
                        } else {
                            return 'This word cannot be removed';
                        }
                    },
                    orderable: false,
                    searchable: false
                }
            ],
            retrieve: true,
            paging: true,
            pageLength: 10,
            lengthMenu: [[10, 25, 50, 100, -1], [10, 25, 50, 100, "All"]],
            ordering: true,
            order: [0, 'asc'],
            language: {
                info: "Showing _START_ to _END_ of _TOTAL_ words."
            }
        });
    },
    validateNewWord: function (newWord) {
        return $('#WordsTable').DataTable().data().filter(w => w.Text === newWord).any();
    },
    removeWord: function (word) {
        $.ajax({
            url: `/api/Word/Delete?word=${word}`,
            type: 'DELETE',
            success: function () {
                $('#WordsTable').DataTable().ajax.reload();
            }
        });
    },
    reloadDataTable: function (dataTableId) {
        $(`#${dataTableId}`).DataTable().ajax.reload();
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
            $('.animation-container').prepend('<img id="Assassin" class="img-endgame img-assassin" src="/images/assassin.png" width="0" height="0" />');

            let spyColour = winningTeam === 'blue' ? 'red' : 'blue';
            $('.animation-container').prepend(`<img id="Spy" class="img-endgame img-spy" src="/images/${spyColour}-man.png" width="0" height="0" />`);
            $('.animation-container').prepend('<img id="Bullet" class="img-endgame img-bullet" src="/images/bullet.png" width="10" height="5" />')

            masterTimeline.add(gsap.timeline().to('#Spy', {
                duration: 0.8,
                width: '20vh',
                height: '40vh',
                opacity: 1,
            }));
            masterTimeline.add(gsap.timeline().to('#Assassin', {
                duration: 1.6,
                width: '20vh',
                height: '40vh',
                rotationY: 360,
                opacity: 1,
                onComplete: () => $('#Bullet').css('opacity', 1)
            }));
            masterTimeline.add(gsap.timeline().to('#Bullet', {
                duration: 0.3,
                opacity: 1,
                x: -Math.max(document.documentElement.clientWidth, window.innerWidth || 0) / 2.6,
                onComplete: () => setTimeout(() => $('#Bullet').remove(), 50)
            }));
            masterTimeline.add(gsap.timeline().to('#Spy', {
                delay: 0.1,
                duration: 0.4,
                width: '20vh',
                height: '40vh',
                opacity: 0
            }));
            masterTimeline.add(gsap.timeline().to('#Assassin', {
                delay: 0.25,
                duration: 0.6,
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