import { GetUserDto } from "../dto";
import userService from "../services/userService";

const userInfos = [] as GetUserDto[]


/**
 * 获取用户信息列表
 * @param userIds 
 * @returns 
 */
async function GetUserInfos(userIds: string[]) {
    const nonExistentUserIds = [] as string[];

    userIds.forEach(userId => {
        const userInfo = userInfos.find(info => info.id === userId);
        if (!userInfo) {
            nonExistentUserIds.push(userId);
        }
    });

    if (nonExistentUserIds.length > 0 || userInfos.length === 0) {
        const result = await userService.list(nonExistentUserIds) as GetUserDto[]
        result.forEach(x=>{
            const userInfo = userInfos.find(s=>s.id === x.id);
            if(!userInfo){
                userInfos.push(x)
            }
        });
    }

    return userInfos.filter((user) =>userIds.includes(user.id));
}

var group ={} as any;

function setGroup(value:any){
    group = value;
}

function getGroup(){
    return group;
}

export {
    GetUserInfos,
    setGroup,
    getGroup,
}