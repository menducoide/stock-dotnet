define({ "api": [
  {
    "success": {
      "fields": {
        "Success 200": [
          {
            "group": "Success 200",
            "optional": false,
            "field": "varname1",
            "description": "<p>No type.</p>"
          },
          {
            "group": "Success 200",
            "type": "String",
            "optional": false,
            "field": "varname2",
            "description": "<p>With type.</p>"
          }
        ]
      }
    },
    "type": "",
    "url": "",
    "version": "0.0.0",
    "filename": "./wwwroot/main.js",
    "group": "D__FRM_MICROSERVICIOS_ecommerce_stock_dotnet_wwwroot_main_js",
    "groupTitle": "D__FRM_MICROSERVICIOS_ecommerce_stock_dotnet_wwwroot_main_js",
    "name": ""
  },
  {
    "type": "get",
    "url": "article/:articleId",
    "title": "GET stock of an article",
    "name": "GET",
    "group": "Stock",
    "parameter": {
      "fields": {
        "Parameter": [
          {
            "group": "Parameter",
            "type": "String",
            "optional": false,
            "field": "articleId",
            "description": "<p>id of article.</p>"
          }
        ]
      }
    },
    "error": {
      "examples": [
        {
          "title": "400:",
          "content": "HTTP/1.1 400 Bad Request\n{\n  \"code\": 400        \n  \"message\": \"stock not found\"\n}",
          "type": "json"
        },
        {
          "title": "401:",
          "content": "HTTP/1.1 401 Unauthorized\n{\n  \"code\": 401        \n  \"message\": \"Unauthorized\"\n}",
          "type": "json"
        }
      ]
    },
    "version": "0.0.0",
    "filename": "./src/application/restController.cs",
    "groupTitle": "Stock"
  },
  {
    "type": "get",
    "url": ":quantity/article/:articleId",
    "title": "Reserve stock of an article",
    "name": "GET",
    "group": "Stock",
    "parameter": {
      "fields": {
        "Parameter": [
          {
            "group": "Parameter",
            "type": "int",
            "optional": false,
            "field": "quantity",
            "description": "<p>quantity to reserve.</p>"
          },
          {
            "group": "Parameter",
            "type": "String",
            "optional": false,
            "field": "articleId",
            "description": "<p>id of article.</p>"
          }
        ]
      }
    },
    "success": {
      "examples": [
        {
          "title": "Success-Response:",
          "content": "HTTP/1.1 200 OK\n{\n  \"reserveId\": \"5dac7e4bb8b02e0e08505db0\"\n}",
          "type": "json"
        }
      ]
    },
    "error": {
      "examples": [
        {
          "title": "400:",
          "content": "HTTP/1.1 400 Bad Request\n{\n  \"code\": 400        \n  \"message\": \"The quantity to reserve isn't avaliable\"\n}",
          "type": "json"
        },
        {
          "title": "401:",
          "content": "HTTP/1.1 401 Unauthorized\n{\n  \"code\": 401        \n  \"message\": \"Unauthorized\"\n}",
          "type": "json"
        }
      ]
    },
    "version": "0.0.0",
    "filename": "./src/application/restController.cs",
    "groupTitle": "Stock"
  },
  {
    "type": "get",
    "url": ":reserveId",
    "title": "Confirm  stock reserve of an article",
    "name": "GET",
    "group": "Stock",
    "parameter": {
      "fields": {
        "Parameter": [
          {
            "group": "Parameter",
            "type": "String",
            "optional": false,
            "field": "reserveId",
            "description": "<p>id of reserve.</p>"
          }
        ]
      }
    },
    "success": {
      "examples": [
        {
          "title": "Success-Response:",
          "content": "HTTP/1.1 200 OK\n{\n  \"success\": \"true\"\n}",
          "type": "json"
        }
      ]
    },
    "error": {
      "examples": [
        {
          "title": "400:",
          "content": "HTTP/1.1 400 Bad Request\n{\n  \"code\": 400        \n  \"message\": \"The reserve isn't avaliable\"\n}",
          "type": "json"
        },
        {
          "title": "401:",
          "content": "HTTP/1.1 401 Unauthorized\n{\n  \"code\": 401        \n  \"message\": \"Unauthorized\"\n}",
          "type": "json"
        }
      ]
    },
    "version": "0.0.0",
    "filename": "./src/application/restController.cs",
    "groupTitle": "Stock"
  },
  {
    "type": "post",
    "url": "article/:articleId",
    "title": "Update stock of an article",
    "name": "Update",
    "group": "Stock",
    "parameter": {
      "fields": {
        "Parameter": [
          {
            "group": "Parameter",
            "type": "String",
            "optional": false,
            "field": "articleId",
            "description": "<p>id of article.</p>"
          }
        ]
      }
    },
    "error": {
      "examples": [
        {
          "title": "400:",
          "content": "HTTP/1.1 400 Bad Request\n{\n  \"code\": 400        \n  \"message\": [{\"key\": \"value\"}]\n}",
          "type": "json"
        },
        {
          "title": "401:",
          "content": "HTTP/1.1 401 Unauthorized\n{\n  \"code\": 401        \n  \"message\": \"Unauthorized\"\n}",
          "type": "json"
        }
      ]
    },
    "version": "0.0.0",
    "filename": "./src/application/restController.cs",
    "groupTitle": "Stock"
  }
] });
