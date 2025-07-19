package com.example.model;

public class OrderItem {
    private Product product;
    private int quantity;
    private int finalQuantity;

    public OrderItem(Product product, int quantity) {
        this.product = product;
        this.quantity = quantity;
        this.finalQuantity = quantity; // Default to original quantity
    }

    public Product getProduct() {
        return product;
    }

    public int getQuantity() {
        return quantity;
    }

    public int getFinalQuantity() {
        return finalQuantity;
    }

    public void setFinalQuantity(int finalQuantity) {
        this.finalQuantity = finalQuantity;
    }

    public int getSubtotal() {
        return product.getUnitPrice() * quantity;
    }
}
