docker build -t wall .
docker run -it --rm -v %cd%:/app -v /app/node_modules -p 3000:3000 -e CHOKIDAR_USEPOLLING=true wall