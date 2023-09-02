import { Component } from 'react'
import './index.scss'

interface ModalProps {
    isOpen: boolean;
    onClose: () => void;
    height?: number;
    width?: number;
    style?: any;
    title: string;
    children?: any;
}

export default class Modal extends Component<ModalProps> {
    handleClose = () => {
        const { onClose } = this.props;
        onClose();
    }

    render() {
        const { isOpen } = this.props;

        if (!isOpen) {
            return null;
        }

        return (
            <div className="modal">
                <div className="modal-overlay" onClick={this.handleClose}></div>
                <div  style={{
                height: this.props.height,
                width: this.props.width,
                ...this.props.style
            }} className="modal-content">
                    <span className="close" onClick={this.handleClose}>&times;</span>
                    <h2>{this.props.title}</h2>
                    {this.props.children}
                </div>
            </div>
        );
    }
}