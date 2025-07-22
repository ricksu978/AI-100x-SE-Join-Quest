package com.example.model;

public class Product {
    private String name;
    private int unitPrice;
    private String category;

    public Product(String name, int unitPrice) {
        this.name = name;
        this.unitPrice = unitPrice;
    }

    public Product(String name, int unitPrice, String category) {
        this.name = name;
        this.unitPrice = unitPrice;
        this.category = category;
    }

    public String getName() {
        return name;
    }

    public int getUnitPrice() {
        return unitPrice;
    }

    public String getCategory() {
        return category;
    }
}
