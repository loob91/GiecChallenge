FROM node:14-alpine AS builder
ENV NODE_ENV production
# Add a work directory
WORKDIR /src-frontend
# Cache and Install dependencies
COPY ./frontend/package.json .
COPY ./frontend/package-lock.json .
ARG REACT_APP_API_URL
ENV REACT_APP_API_URL=$REACT_APP_API_URL
RUN npm install
# Copy app files
COPY ./frontend/ .
RUN mv ./public/locales/fr ./public/locales/FR
RUN mv ./public/locales/en ./public/locales/EN
# Build the app
CMD ["npm", "run", "docker"]