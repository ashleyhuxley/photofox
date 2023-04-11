package com.fionasapphire.photofox.storage

import com.microsoft.azure.storage.CloudStorageAccount
import com.microsoft.azure.storage.table.CloudTable

open class StorageBase(private val connectionString: String, private val tableName: String) {

    private fun getAccount(): CloudStorageAccount {
        return CloudStorageAccount.parse(connectionString)
    }

    protected fun getTableReference(): CloudTable {
        val account = getAccount()
        val client = account.createCloudTableClient()
        return client.getTableReference(tableName)
    }
}