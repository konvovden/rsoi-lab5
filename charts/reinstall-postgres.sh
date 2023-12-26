helm uninstall postgresdb
kubectl delete pvc data-postgresdb-postgresql-0
helm install postgresdb --set auth.postgresPassword=PaSSw0rdAdmin oci://registry-1.docker.io/bitnamicharts/postgresql
