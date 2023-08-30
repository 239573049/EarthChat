import { Button } from '@douyinfe/semi-ui';
import { Component } from 'react'
import { IconMoon, IconSun } from '@douyinfe/semi-icons';

const body = document.body;

export default class Theme extends Component {
    
    changeTheme = (ev: any) => {

        var d = document as any;
        d.documentElement.style.setProperty('--x', ev.clientX + 'px')
        d.documentElement.style.setProperty('--y', ev.clientY + 'px')

        if (body.hasAttribute('theme-mode')) {
            d.startViewTransition(() => {
                body.removeAttribute('theme-mode');
            });
        } else {
            d.startViewTransition(() => {
                body.setAttribute('theme-mode', 'dark');
            });
        }
    }

    render() {
        return (
            <Button theme='borderless' onClick={(e) => this.changeTheme(e)} icon={body.hasAttribute('theme-mode') ? <IconMoon size='large'></IconMoon> : <IconSun></IconSun>}></Button>
        )
    }
}
