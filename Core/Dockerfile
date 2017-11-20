FROM microsoft/aspnetcore:2.0
ARG source
WORKDIR /app
EXPOSE 80
COPY ${source:-obj/Docker/publish} .
ENTRYPOINT ["dotnet", "Core.dll"]
RUN chmod +x ./entrypoint.sh
CMD /bin/bash ./entrypoint.sh
