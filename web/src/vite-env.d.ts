interface ImportMetaEnv {
    readonly VITE_API: string
    readonly VITE_GITEE_ID:string;
    readonly MODE:string;
  }
  
  interface ImportMeta {
    readonly env: ImportMetaEnv
  }