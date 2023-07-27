import { Component } from "react";

class Mention extends Component {

    state = {
        image: [],
        text: '',
        maxlength: 1000,
    }

    handleInput(e) {
        const editorContainer = document.getElementById('editor-container');

        editorContainer.appendChild(e.target.innerHTML);
    }

    getValue() {
        var element = document.getElementById("editor-container");
        const base64 = [];
        for (let i = 0; i < element.children.length; i++) {
            const item = element.children[i].src;
            if (item) {
                // 去掉base64的前缀
                base64.push(item.substring(item.indexOf(',') + 1));
            }
        }
        return {
            base64,
            content: element.innerText,
        }
    }

    setValue(value) {
        // 如果value超过了maxlength，就截取
        if (value && value.length > this.state.maxlength) {
            value = value.substring(0, this.state.maxlength);
        }
        const editorContainer = document.getElementById('editor-container');
        editorContainer.innerHTML = value;
    }


    handlePaste(e) {
        e.preventDefault();
        
        const text = e.clipboardData.getData('text/plain');
        const image = e.clipboardData.items[0];
        if (image && image.type.includes('image')) {

            const reader = new FileReader();
            reader.onload = (event) => {
                const img = document.createElement('img');
                img.src = event.target.result;
                img.height = 50;
                const editorContainer = document.getElementById('editor-container');
                editorContainer.innerHTML += img.outerHTML;
            };
            reader.readAsDataURL(image.getAsFile());
        } else {
            const editorContainer = document.getElementById('editor-container');
            editorContainer.innerHTML += text;
            // 如果editorContainer的长度超过了maxlength，就截取
            if (editorContainer.innerText.length > this.state.maxlength) {
                editorContainer.innerText = editorContainer.innerText.substring(0, this.state.maxlength);
            }
        }
    }

    onKeyDown(e){
        // 如果按下了回车键，就阻止默认行为
        if (e.keyCode === 13) {
            e.preventDefault();
            // 触发onSubmit事件
            this.props.onSubmit();
            
        }
    }

    render() {
        return (
            <div
                contentEditable
                id="editor-container"
                onInput={(e) => this.handleInput(e)}
                onPaste={(e) => this.handlePaste(e)}
                onKeyDown={(e)=>this.onKeyDown(e)}
                style={{
                    padding: '10px',
                    minHeight: '140px',
                    maxHeight: '140px',
                    // 去掉获取焦点的边框
                    outline: 'none',
                }}
            >
            </div>
        )
    }
}

export default Mention;