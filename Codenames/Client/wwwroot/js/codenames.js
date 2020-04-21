﻿window.codenames = {
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
    runAfterTimeout(functionToRun, param, timeout) {
        setTimeout(() => this[functionToRun](param), timeout);
    }
}