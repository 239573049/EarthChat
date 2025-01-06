import { createSystemAction, SystemAction } from './action';
import { createWithEqualityFn } from 'zustand/traditional';
import { initialState, ThemeState } from './initialState';
import { StateCreator } from 'zustand/vanilla';
import shallow from 'zustand/shallow';
import { createDevtools } from '../middleware/createDevtools'
;
export type SystemStore = ThemeState 
  & SystemAction;

const createStore: StateCreator<SystemStore, [['zustand/devtools', never]]> = (...parameters) => ({
    ...initialState,
    ...createSystemAction(...parameters),
});

const devtools = createDevtools('file');

export const useSystemStore = createWithEqualityFn<SystemStore>()(devtools(createStore), shallow);