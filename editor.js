const vditor = new Vditor("vditor", {preview:{maxWidth:800}});

function getFile() {
    fetch("http://127.0.0.1:9091/hello.md")
        .then((response) => response.text())
        .then((data) => {
          alert( document.getElementById("toolbar").offsetWidth)

            vditor.setValue(data, (clearStack = false));
        });
}

//获取编辑器内容
ameGetValue = () => {
    return vditor.getValue();
};

//在焦点处插入内容
ameInsertValue = (value, render) => {
    console.log(render);

    vditor.insertValue(value, render == "true" ? true : false);
};

//聚焦到编辑器
ameFocus = () => {
    vditor.focus();
};

//失焦
ameBlur = () => {
    vditor.blur();
};

//禁用
ameDisabled = () => {
    vditor.disabled();
};

//解除编辑器禁用
ameEnable = () => {
    vditor.enable();
};

//选中从 start 开始到 end 结束的字符串
ameSetSelection = (start, end) => {
    vditor.setSelection(start, end);
};

//返回选中的字符串
ameGetSelection = () => {
    return vditor.getSelection();
};

//设置编辑器内容
ameSetValue = (value) => {
    vditor.setValue(value);
};

//获取焦点位置
ameGetCursorPosition = () => {
    return JSON.stringify(vditor.getCursorPosition());
};

//删除选中内容
ameDeleteValue = () => {
    vditor.deleteValue();
};

//更新选中内容
ameUpdateValue = (value) => {
    vditor.updateValue(value);
};

//清除缓存
ameClearCache = () => {
    vditor.clearCache();
};

//禁用缓存
ameDisabledCache = () => {
    vditor.disabledCache();
};

//启用缓存
ameEnableCache = () => {
    vditor.enableCache();
};

//设置预览模式
ameSetPreviewMode = (mode) => {
    //alert(mode);
    console.log(mode);

    vditor.setPreviewMode(mode);
};

//设置模式
ameSetWysiwyg = (mode) => {
    vditor.setWysiwyg(mode);
};

//消息提示
ameTip = (text, time) => {
    vditor.tip(text, time);
};

ameUndo = () => {
    vditor.undo();
};

ameRedo = () => {
    vditor.redo();
};

ameSetBold = () => {
    vditor.setBold();
};

ameSetH1 = () => {
    vditor.setH1();
};

ameSetH2 = () => {
    vditor.setH2();
};

ameSetH3 = () => {
    vditor.setH3();
};

ameSetH4 = () => {
    vditor.setH4();
};

ameSetH5 = () => {
    vditor.setH5();
};

ameSetH6 = () => {
    vditor.setH6();
};

ameSetItalic = () => {
    vditor.setItalic();
};

ameSetStrike = () => {
    vditor.setStrike();
};

ameSetLine = () => {
    vditor.setLine();
};

ameSetQuote = () => {
    vditor.setQuote();
};

ameSetList = () => {
    vditor.setList();
};

ameSetOrdered = () => {
    vditor.setOrdered();
};

ameSetCheck = () => {
    vditor.setCheck();
};

ameSetCode = () => {
    vditor.setCode();
};

ameSetInlineCode = () => {
    vditor.setInlineCode();
};

ameSetLink = () => {
    vditor.setLink();
};

ameSetTable = () => {
    vditor.setTable();
};

ameGetHtml = () => {
    // console.log(vditor.getHTML());

    // let str = vditor.getHTML();

    // let str1 = str.replace(new RegExp("\\u003C","gm"),"<")

    // return str1
    return vditor.getHTML();
    // vditor.getHTML().then(res => {
    //   ameBridge.getHtml(res)
    // })
};

ameHtml2md = (value) => {
    return vditor.html2md(value);
    // vditor.html2md(value).then(res => {
    //   ameBridge.html2md(res)
    // })
};
