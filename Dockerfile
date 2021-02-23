from node:alpine
WORKDIR /app/
COPY . ./

# Install dependencies
RUN yarn
# Build Project
RUN yarn run build
COPY src/wrapper.html /app/build/src/wrapper.html
CMD [ "yarn", "run" , "start" ]