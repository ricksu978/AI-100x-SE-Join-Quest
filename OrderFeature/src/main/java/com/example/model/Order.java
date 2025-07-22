package com.example.model;

import java.util.List;
import java.util.ArrayList;

public class Order {
    private List<OrderItem> items;
    private int originalAmount;
    private int discount;
    private int totalAmount;

    public Order() {
        this.items = new ArrayList<>();
    }

    public void addItem(OrderItem item) {
        items.add(item);
    }

    public List<OrderItem> getItems() {
        return items;
    }

    public OrderItem getItemByProductName(String productName) {
        for (OrderItem item : items) {
            if (item.getProduct().getName().equals(productName)) {
                return item;
            }
        }
        return null;
    }

    public int getOriginalAmount() {
        return originalAmount;
    }

    public int getDiscount() {
        return discount;
    }

    public int getTotalAmount() {
        return totalAmount;
    }

    public void setOriginalAmount(int originalAmount) {
        this.originalAmount = originalAmount;
    }

    public void setDiscount(int discount) {
        this.discount = discount;
    }

    public void setTotalAmount(int totalAmount) {
        this.totalAmount = totalAmount;
    }
}
