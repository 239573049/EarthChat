
import { create } from 'zustand';

type GlobalStore = {
    theme: string | 'dark' | 'light' | "auto";
    setTheme: (theme: string) => void;
};

export const useGlobalStore = create<GlobalStore>((set) => ({
    theme: 'auto',
    setTheme: (theme) => set({ theme }),
}));