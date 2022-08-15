# ETL | .NET console app

Basic ETL service which allows you to process files that users save in the specific folder on the disk. A file can be either in TXT or CSV format with the following content:
```
<first_name: string>, <last_name: string>, <address: string>, <payment: decimal>, <date: date>, <account_number: long>, <service: string>
```

Result in a specified format:
```
[{
  "city": "string",
  "services": [{
    "name": "string",
    "payers": [{
      "name": "string",
      "payment": "decimal",
      "date": "date",
      "account_number": "long"
    }],
    "total": "decimal"
  }],
  "total": "decimal"
}]
```
