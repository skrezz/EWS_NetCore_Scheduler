FROM node:16-alpine as build
ENV NODE_OPTIONS=--max_old_space_size=2048
WORKDIR /app
COPY package.json /app/package.json
RUN npm install --only=prod
COPY . /app
RUN npm run build
FROM nginx:1.23
COPY --from=build /app/build /usr/share/nginx/html
RUN rm /etc/nginx/conf.d/default.conf
CMD ["nginx", "-g", "daemon off;"]
