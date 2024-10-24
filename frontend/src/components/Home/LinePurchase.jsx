import React from 'react';
import {Link} from 'react-router-dom'
import { FaTrash } from 'react-icons/fa';

export const LinePurchase = ({ purchaseUser, onChange }) => {    
    return (
      <>
        <div className='flex'>
          <div className='form-group width-20 inline-flex'>
            <Link to={`/purchase/${purchaseUser.id}`}>{new Date(Date.parse(purchaseUser.datePurchase)).toLocaleString("fr-FR", {month: '2-digit',day: '2-digit',year: 'numeric'})}</Link>
          </div>
          <div className='form-group width-20 inline-flex'>{parseFloat(purchaseUser.cO2Cost).toFixed(2)}</div>
          <div className='form-group width-20 inline-flex'>{parseFloat(purchaseUser.waterCost).toFixed(2)}</div>
          <div>
            <a onClick={() => onChange(purchaseUser.id)} href="#/" className='btn btn-reverse btn-block'>
                <FaTrash />
            </a>
          </div>
        </div>
      </>
    );
  };