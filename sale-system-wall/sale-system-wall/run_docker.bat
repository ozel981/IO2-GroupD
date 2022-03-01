docker build -t wall:prod -f Dockerfile.prod . 
docker run --rm -d -p 3000:80 wall:prod       