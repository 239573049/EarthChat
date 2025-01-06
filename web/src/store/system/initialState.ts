
export interface ThemeState {
    theme: 'light' | 'dark' | 'auto';

}

export const initialState: ThemeState = {
    theme: localStorage.getItem('token-ai-theme') as 'light' | 'dark' | 'auto' || 'auto',
}