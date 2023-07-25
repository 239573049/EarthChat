FROM nginx:stable-alpine AS base
WORKDIR /app
EXPOSE 80

FROM node:16-alpine3.15 AS build
WORKDIR /src
COPY . .
WORKDIR "/src"
RUN npm config set registry https://registry.npm.taobao.org
RUN yarn
RUN yarn build

FROM base AS chat-web
WORKDIR /wwwroot
COPY --from=build /src/dist /wwwroot/
COPY conf.d/default.conf /etc/nginx/conf.d/default.conf