interface ImportMetaEnv {
    readonly VITE_API: string
    readonly VITE_GITEE_ID:string;
  }
  
  interface ImportMeta {
    readonly env: ImportMetaEnv
  }