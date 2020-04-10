window.codenames = {
    setFocus: function (id) {
        document.getElementById(id).focus();
    },
    blurElement: function (id) {
        document.getElementById(id).blur();
    },
    scrollToBottomOfElement: function (id) {
        let element = document.getElementById(id);
        element.scrollTop = element.scrollHeight;
    }
}