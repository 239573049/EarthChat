
import { StateCreator } from 'zustand/vanilla';
import { SystemStore } from './store';
export interface SystemAction {
    switchTheme: (theme: 'light' | 'dark' | 'auto') => void;
}


export const createSystemAction: StateCreator<
    SystemStore,
    [['zustand/devtools', never]],
    [],
    SystemAction
> = (set, get) => ({
    switchTheme: (theme) => {
        if (theme === get().theme) {
            return;
        }
        set((state) => {
            return {
                ...state,
                theme,
            };
        });
        localStorage.setItem('token-ai-theme', theme);
    },
});