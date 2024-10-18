
import { create } from 'zustand';


interface UserStore {
    /**
     * 是否登录
     */
    isLogin: boolean;

    /**
     * 用户信息
     */
    userInfo: {
    } | null;

    /**
     * 登录
     */
    login: (userInfo: {}) => void;

    /**
     * 退出登录
     */
    logout: () => void;
}

export const useUserStore = create<UserStore>((set) => ({
    isLogin: false,
    userInfo: null,
    login: (userInfo) => set({ isLogin: true, userInfo }),
    logout: () => set({ isLogin: false, userInfo: null }),
}));