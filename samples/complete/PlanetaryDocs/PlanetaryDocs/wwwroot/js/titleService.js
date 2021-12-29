window.titleService = {
    titleRef: null,
    setTitle: (title) => {
        var _self = window.titleService;
        if (_self.titleRef == null) {
            _self.titleRef = document.getElementsByTagName("title")[0];
        }
        setTimeout(() => _self.titleRef.innerText = title, 0);
    }
}