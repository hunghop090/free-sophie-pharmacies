#!/bin/bash
set -e

mongo -u "$MONGO_INITDB_ROOT_USERNAME" -p "$MONGO_INITDB_ROOT_PASSWORD" localhost/admin <<-EOJS
    var rootUser     = '$MONGO_INITDB_ROOT_USERNAME';
    var rootPassword = '$MONGO_INITDB_ROOT_PASSWORD';
    var rootDatabase = '$MONGO_INITDB_DATABASE';
    db = db.getSiblingDB(rootDatabase);
    db.auth(rootUser, rootPassword);

    var _user = 'sophie_user';
    var _passwd = 'Abc#1234';
    var _database = 'sophie_db';

    print('===> Create DB...');
    db = db.getSiblingDB(_database);

    print('===> Create user...');
    db.createUser({
        user: _user,
        pwd: _passwd,
        roles: [
            { role: 'userAdminAnyDatabase', db: rootDatabase },
            { role: 'readWriteAnyDatabase', db: rootDatabase },
            { role: 'dbAdminAnyDatabase', db: rootDatabase },
            { role: 'readWrite', db: $(_js_escape "$MONGO_INITDB_DATABASE") },

            { role: 'readWrite', db: _database },
        ]
    });
EOJS