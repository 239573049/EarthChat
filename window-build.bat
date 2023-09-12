@echo off
chcp 65001

set "currentDir=%~dp0"

node -v > nul 2>&1
if %errorlevel% equ 0 (

    echo 复制完成构建后端项目

    dotnet --version > nul 2>&1

    rem 检查dotnet是覅安装
    if %errorlevel% equ 0 (
        echo dotnet已安装,开始构建后端

        dotnet  publish -c Release --output DevOps/service src/Chat.Service/Chat.Service.csproj

        echo 后端构建完成。

    ) else (
        echo dotnet未安装
    )


    echo 检查已经安装node

    echo 开始构建Window前端项目
    cd /d "%currentDir%/web"

    npm i 
    npm run build

    echo 前端项目构建完成开始复制构建文件
	


    xcopy /Y /E /I "%currentDir%web\dist" "%currentDir%DevOps\service\wwwroot"


) else (
    echo 未安装Node将不执行构建程序
)


pause > nul
