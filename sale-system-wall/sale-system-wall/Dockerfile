FROM node:16-alpine

WORKDIR /app

ENV PATH /app/node_modules/.bin:$PATH

COPY package.json ./
COPY package-lock.json ./
RUN npm ci --unsafe-perm

COPY . ./

ENTRYPOINT ["npm"]
CMD ["run", "start"]
