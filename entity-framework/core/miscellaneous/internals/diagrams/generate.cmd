:: npm install -g @mermaid-js/mermaid-cli
FOR %%f IN (*.mmd) DO mmdc -i %%f -o %%~nf.png
