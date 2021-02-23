from node:alpine
WORKDIR /app/
COPY . ./

# Install dependencies
RUN yarn
# Build Project
RUN yarn run build

CMD [ "yarn", "run" , "start" ]