# How to change the basic auth password:
# 1. Install apache2-utils (Linux) or httpd-tools (RHEL/CentOS)
# 2. Run: htpasswd -c nginx/.htpasswd yourusername
# 3. Rebuild the nginx container: docker-compose build nginx
# 4. Restart: docker-compose up -d nginx

# Default credentials:
# Username: admin
# Password: admin
