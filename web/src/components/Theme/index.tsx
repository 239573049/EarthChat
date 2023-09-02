import { Button } from '@douyinfe/semi-ui';
import { Component } from 'react'
import { IconMoon, IconSun } from '@douyinfe/semi-icons';

const body = document.body;

const theme = localStorage.getItem('theme-mode');
if (theme) {
    if (theme === "light") {
        body.removeAttribute('theme-mode');
    } else if (theme === "dark") {
        body.setAttribute('theme-mode', 'dark');
    }
}

export default class Theme extends Component {

    changeTheme = (ev: any) => {

        var d = document as any;
        d.documentElement.style.setProperty('--x', ev.clientX + 'px')
        d.documentElement.style.setProperty('--y', ev.clientY + 'px')

        if (body.hasAttribute('theme-mode')) {
            d.startViewTransition(() => {
                body.removeAttribute('theme-mode');
                localStorage.setItem('theme-mode', 'light')
            });
        } else {
            d.startViewTransition(() => {
                body.setAttribute('theme-mode', 'dark');
                localStorage.setItem('theme-mode', 'dark')
            });
        }
    }

    render() {
        return (
            <Button theme='borderless' onClick={(e) => this.changeTheme(e)} icon={body.hasAttribute('theme-mode') ? <IconMoon size='large'></IconMoon> : <IconSun></IconSun>}></Button>
        )
    }
}
