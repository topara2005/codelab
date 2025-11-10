FROM nginx:alpine

# Install apache2-utils for htpasswd
RUN apk add --no-cache apache2-utils

# Copy nginx configuration
COPY nginx.conf /etc/nginx/nginx.conf

# Create htpasswd file with default user (admin:admin)
# You can change this by running: htpasswd -c .htpasswd yourusername
RUN htpasswd -cb /etc/nginx/.htpasswd admin admin

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
