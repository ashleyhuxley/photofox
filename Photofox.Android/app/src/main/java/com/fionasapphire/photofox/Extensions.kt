package com.fionasapphire.photofox

import java.time.*
import java.time.format.DateTimeFormatter
import java.util.*

fun Date.asKey(): String {
    return this.toInstant().asKey()
}

fun Instant.asKey(): String {
    val df = DateTimeFormatter.ofPattern("yyyyMMdd").withZone(ZoneId.from(ZoneOffset.UTC))
    return df.format(this)
}

fun Date.atMidnight() : LocalDateTime {
    val localDate = this.toInstant().atZone(ZoneId.systemDefault()).toLocalDate()
    return LocalDateTime.of(localDate, LocalTime.MIDNIGHT);
}