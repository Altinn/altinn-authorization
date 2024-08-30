ARG APP_NAME

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
ARG APP_NAME
COPY . .
WORKDIR /src/apps/${APP_NAME}
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS final
ARG APP_NAME
EXPOSE 5050
COPY --from=build /app .

RUN echo "#!/bin/sh" >> entrypoint.sh
RUN echo "exec dotnet ${APP_NAME}.dll" >> entrypoint.sh
RUN chmod +x entrypoint.sh

ENTRYPOINT ["/entrypoint.sh"]