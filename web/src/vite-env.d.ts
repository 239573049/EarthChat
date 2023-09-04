interface ImportMetaEnv {
    readonly VITE_API: string
    readonly VITE_GITEE_ID:string;
    readonly MODE:string;
    readonly VITE_KEY:string
  }
  
  interface ImportMeta {
    readonly env: ImportMetaEnv
  }