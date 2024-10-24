import React from 'react';
import { useEffect, useState } from 'react';
import { useDispatch } from 'react-redux'
import { useTranslation } from 'react-i18next';
import { ProductSearchBoxPreFill } from './ProductSearchBoxPreFill';
import { FaTrash } from 'react-icons/fa';

export const LineProductPreFill = ({ product, nameSelect, onChange, toDelete }) => {
    const [selectedValue, setSelectedValue] = useState(product.productId);
    const [quantity, setQuantity] = useState(product.quantity);
    const [price, setPrice] = useState(product.price);
    const dispatch = useDispatch()
    const { t } = useTranslation(["LineProduct"]);

    useEffect(() => {  
      onChange({
        id: nameSelect,
        quantity,
        price,
        selectedValue,
        translation: product.product,
        currencyIsoCode: product.currencyIsoCode
      });
    }, [dispatch, onChange, quantity, selectedValue, price, product.currencyIsoCode, product.product, nameSelect]);
    
    return (
      <>
        <div className='flex'>
          <div>
            <fieldset key={`${nameSelect}fieldset`} className='mtop-10'>
              <legend>{t("product")}</legend>
              <div>
                  <input type="text" name={`${nameSelect}quantity`} className="width-40 inlineflex form-control" id={`${nameSelect}quantity`} value={quantity} placeholder={t("quantity")} onChange={(e) => { setQuantity(e.target.value); }} />
                  <input type="text" name={`${nameSelect}price`} className="width-40 inlineflex form-control" id={`${nameSelect}price`} value={price} placeholder={t("price")} onChange={(e) => { setPrice(e.target.value); }} />
              </div>
              <input type="hidden" key={`${nameSelect}currency`} name={`${nameSelect}currency`} className="width-100 inlineflex form-control" id={`${nameSelect}currency`} onChange={() => {}} value={product.currencyIsoCode} />
              <ProductSearchBoxPreFill key={`${nameSelect}searchbox`} className='mbottom-10' preSelectedValue={product.productId} preSelectedInputValue={product.translation} nameSelect={nameSelect} onChange={(e) => setSelectedValue(e)}/>
            </fieldset>
          </div>
          <div>
            <a onClick={() => toDelete(product.id)} href="#/" className='btn btn-reverse btn-block'>
                <FaTrash />
            </a>
          </div>
        </div>
      </>
    );
  };