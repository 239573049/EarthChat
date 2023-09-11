import { Avatar, Tooltip } from '@douyinfe/semi-ui';
import { ClipboardEvent, KeyboardEvent, Component } from 'react';
import './index.scss'

interface MentionProps {
  onSubmit: () => void;
  users: any[],
  style: any;
}

interface MentionState {
  image: string[];
  text: string;
  maxlength: number;
  visible: boolean;
}

class Mention extends Component<MentionProps, MentionState> {

  state: MentionState = {
    image: [],
    text: '',
    maxlength: 1000,
    visible: false,
  };

  handleInput = (e: any) => {
    if (e.nativeEvent.data === '@') {
      this.setState({ visible: true })
    }
    const editorContainer = document.getElementById('editor-container');
    if (editorContainer && e.target) {
      try {
        editorContainer.appendChild((e.target as HTMLDivElement).innerHTML as any);
      } catch {

      }
    }
  };

  getValue = () => {
    const element = document.getElementById("editor-container");
    const base64: string[] = [];
    if (element) {
      for (let i = 0; i < element.children.length; i++) {
        const item = (element.children[i] as HTMLImageElement).src;
        if (item) {
          base64.push(item.substring(item.indexOf(',') + 1));
        }
      }
    }
    return {
      base64,
      content: element?.innerText || '',
    };
  };

  setValue = (value: string) => {
    try {
      if (value && value.length > this.state.maxlength) {
        value = value.substring(0, this.state.maxlength);
      }
      const editorContainer = document.getElementById('editor-container');
      if (editorContainer) {
        editorContainer.innerHTML = value;
      }
    } catch {

    }
  };

  handlePaste = (e: ClipboardEvent<HTMLDivElement>) => {
    e.preventDefault();
    const text = e.clipboardData.getData('text/plain');
    const image = e.clipboardData.items[0];
    if (image && image.type.includes('image')) {
      const reader = new FileReader();
      reader.onload = (event) => {
        const img = document.createElement('img');
        img.src = (event.target as FileReader).result as string;
        img.height = 50;
        const editorContainer = document.getElementById('editor-container');
        if (editorContainer) {
          editorContainer.innerHTML += img.outerHTML;
        }
      };
      reader.readAsDataURL(image.getAsFile()!);
    } else {
      const editorContainer = document.getElementById('editor-container');
      if (editorContainer) {
        editorContainer.innerHTML += text;
        if (editorContainer.innerText.length > this.state.maxlength) {
          editorContainer.innerText = editorContainer.innerText.substring(0, this.state.maxlength);
        }
      }
    }
  };

  onKeyDown = (e: KeyboardEvent<HTMLDivElement>) => {
    // 如果是快捷键则调用发送。
    if (e.keyCode === 13) {
      e.preventDefault();
      this.props.onSubmit();
    }
  };

  renderUsers() {
    return <div>
      {this.props.users.map(x => {
        return <div>
          <div className='mention-item' style={{
            float: 'left',
            width:'110px',
          }}>
            <Avatar src={x.avatar} size="extra-small" />
            <span>
              {x.name}
            </span>
          </div>
        </div>
      })}
    </div>
  }

  render() {
    const { visible } = this.state;
    return (
      <div
        contentEditable
        id="editor-container"
        onInput={this.handleInput}
        onPaste={this.handlePaste}
        onKeyDown={this.onKeyDown}
        style={{
          padding: '10px',
          outline: 'none',
          overflow: 'auto',
          ...this.props.style,
        }}
      >
        <Tooltip
          onClickOutSide={() => this.setState({ visible: false })}
          content={this.renderUsers()}
          className='mention'
          trigger="custom"
          visible={visible}
        >
        </Tooltip>
      </div>
    );
  }
}

export default Mention;
