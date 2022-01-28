window.markdownExtensions = {
    toHtml: (txt, target) => {
        const area = document.createElement("textarea");
        setTimeout(() => {
            area.innerHTML = txt;
            target.innerHTML = area.value;
        }, 0);
    },
    setText: (id, txt, target) => {
        target.value = txt;
        target.oninput = () => window.markdownExtensions.getText(id, target);
    },
    getText: (id, target) => DotNet.invokeMethodAsync(
        'PlanetaryDocs',
        'UpdateTextAsync',
        id,
        target.value)
};